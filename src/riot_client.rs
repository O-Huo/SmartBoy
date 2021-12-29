use riven::consts::PlatformRoute;
use riven::consts::RegionalRoute;
use riven::RiotApi;
use riven::consts::Tier;
use riven::models::summoner_v4::Summoner;
use std::env;


pub async fn get_summoner_for_user(user_name: String) -> Summoner {
    let riot_api = RiotApi::new(env::var("RAPI").unwrap_or_default());
    let summoner = riot_api
        .summoner_v4()
        .get_by_summoner_name(PlatformRoute::NA1, &user_name)
        .await
        .unwrap()
        .unwrap();
    return summoner;
}

pub async fn get_rank(id: String) -> Tier {
    let riot_api = RiotApi::new(env::var("RAPI").unwrap_or_default());
    let entries = riot_api
        .league_v4()
        .get_league_entries_for_summoner(PlatformRoute::NA1, &id)
        .await
        .unwrap();
    if entries.len() == 0 {
        return Tier::IRON;
    }
    return entries[0].tier.unwrap_or(Tier::IRON);
}


pub async fn get_lose_streak(riot_id: String, last_query_time: i64, current_streak: i32 ) -> i32 {
    println!("get lose streak for user {}", riot_id);
    let riot_api = RiotApi::new(env::var("RAPI").unwrap_or_default());
    let matches = riot_api
        .match_v5()
        .get_match_ids_by_puuid(RegionalRoute::AMERICAS,&riot_id, None, None, None, Some(last_query_time), None, None)
        .await
        .unwrap();
    let mut lose_streak = 0;
    for match_id in matches {
        println!("get match {}", match_id);
        let game = riot_api.match_v5().get_match(RegionalRoute::AMERICAS, &match_id)
            .await
            .unwrap()
            .unwrap();
        for participant in game.info.participants {
            if participant.puuid == riot_id {
                if participant.win {
                    return lose_streak;
                } else {
                    lose_streak += 1;
                }
            }
        }
    }
    return lose_streak + current_streak;
}
