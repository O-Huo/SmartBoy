use crate::riot_client::get_summoner_for_user;
use crate::riot_client::get_lose_streak;
use crate::riot_client::get_tier;
use std::time::SystemTime;
use serde::Deserialize;
use serde::Serialize;
use std::hash::Hash;
use riven::consts::Tier;

#[derive(Serialize, Deserialize, Clone)]
pub struct User {
    pub user_name: String,
    pub riot_id: String,
    pub riot_puuid_id: String,
    pub tg_id: u64,
    pub tg_name: String,
    pub last_query_time: i64,
    pub lose_streak: i32,
    pub tier: Tier,
}

impl PartialEq for User {

    fn eq(&self, other: &Self) -> bool {
        return self.tg_id.eq(&other.tg_id)
    }
}

impl Eq for User {}

impl Hash for User {
    fn hash<H: std::hash::Hasher>(&self, state: &mut H) {
        self.tg_id.hash(state)
    }
}




impl User {
    pub async fn new(user_name: String, tg_id: u64, tg_name: String) -> Self {
        let summoner = get_summoner_for_user(user_name.clone()).await;
        let riot_id = summoner.id;
        let puuid = summoner.puuid;
        let (lose_streak, last_query_time) = get_lose_streak(puuid.clone(), 0, 0).await;
        Self {
            user_name,
            lose_streak,
            tier: get_tier(riot_id.clone()).await,
            riot_id,
            riot_puuid_id: puuid,
            tg_id,
            tg_name,
            last_query_time
        }
    }

    pub async fn refresh(& mut self) {
        let summoner = get_summoner_for_user(self.user_name.clone()).await;
        self.riot_id = summoner.id;
        self.riot_puuid_id = summoner.puuid;
        self.last_query_time = 0;
        self.lose_streak = 0;
    }

    pub async fn update(& mut self) -> bool {
        println!("Update {}", &self.tg_name);
        let (new_streak, new_query_time) =
            get_lose_streak(self.riot_puuid_id.clone(),
                            self.last_query_time, self.lose_streak).await;
        let tier = get_tier(self.riot_id.clone()).await;
        self.last_query_time = new_query_time;
        let mut updated = false;
        if new_streak != self.lose_streak {
            self.lose_streak = new_streak;
            updated = true;
        }
        if tier != self.tier {
            self.tier = tier;
            updated = true;
        }
        return updated;
    }
}