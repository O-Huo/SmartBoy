use riven::consts::PlatformRoute;
use riven::consts::RegionalRoute;
use riven::{RiotApi, RiotApiConfig};
use riven::consts::Tier;
use riven::models::summoner_v4::Summoner;
use std::env;
use std::time::{Duration, SystemTime};
use lazy_static::lazy_static;
use riven::reqwest::ClientBuilder;
use riven::reqwest::header::HeaderValue;
use tokio::time::timeout;
use warp::http::HeaderMap;

lazy_static! {
    static ref API: RiotApi = RiotApi::new(RiotApiConfig::with_key(env::var("RAPI")
        .unwrap_or_default()));
    // RiotApi::new((|| {
    //     let mut default_headers = HeaderMap::new();
    //     default_headers.insert(
    //         RiotApiConfig::RIOT_KEY_HEADER,
    //         HeaderValue::from_bytes(env::var("RAPI").unwrap_or_default().as_ref()).unwrap()
    //     );
    //     let client_builder = ClientBuilder::new();
    //     let config = RiotApiConfig::with_client_builder(
    //         client_builder
    //         .default_headers(default_headers)
    //         .timeout(Duration::from_secs(5))
    //         .connect_timeout(Duration::from_secs(5))
    //         .connection_verbose(true)
    //         .tcp_keepalive(None)
    //         // .pool_max_idle_per_host(0)
    //         .pool_idle_timeout(None)
    //         .no_gzip()
    //     );
    //     config
    // })());
}


pub async fn get_summoner_for_user(user_name: String) -> Summoner {
    let summoner = API
        .summoner_v4()
        .get_by_summoner_name(PlatformRoute::NA1, &user_name)
        .await
        .unwrap()
        .unwrap();
    return summoner;
}

pub async fn get_tier(id: String) -> Tier {
    let entries = API
        .league_v4()
        .get_league_entries_for_summoner(PlatformRoute::NA1, &id)
        .await
        .unwrap();
    if entries.len() == 0 {
        return Tier::IRON;
    }
    return entries[0].tier.unwrap_or(Tier::IRON);
}


pub async fn get_lose_streak(riot_id: String, last_query_time: i64,
                             current_streak: i32) -> (i32, i64) {
    println!("get lose streak for user {}", riot_id);
    let matches =
        API
            .match_v5()
            .get_match_ids_by_puuid(RegionalRoute::AMERICAS,&riot_id, None, None, None, Some
                (last_query_time), None, None)
            .await
            .unwrap();
    let mut lose_streak = 0;
    let mut current_query_time = last_query_time;
    for match_id in matches {
        println!("get match {}", match_id);
        let game = API.match_v5().get_match(RegionalRoute::AMERICAS, &match_id)
            .await
            .unwrap()
            .unwrap();
        let game_end_timestamp = game.info.game_end_timestamp.unwrap() / 1000;
        if game_end_timestamp + 1 > current_query_time {
            current_query_time = game_end_timestamp + 1;
        }
        for participant in game.info.participants {
            if participant.puuid == riot_id {
                if participant.win {
                    return (lose_streak, current_query_time);
                } else {
                    lose_streak += 1;
                }
            }
        }
    }
    return (lose_streak + current_streak, current_query_time);
}
