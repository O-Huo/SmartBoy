use crate::riot_client::get_summoner_for_user;
use crate::riot_client::get_lose_streak;
use crate::riot_client::get_rank;
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
        Self {
            user_name,
            lose_streak: get_lose_streak(puuid.clone(), 0, 0).await,
            tier: get_rank(riot_id.clone()).await,
            riot_id,
            riot_puuid_id: puuid,
            tg_id,
            tg_name,
            last_query_time: SystemTime::now().duration_since(SystemTime::UNIX_EPOCH).expect("").as_secs() as i64,
        }
    }

    pub async fn update(& mut self) -> bool {
        self.last_query_time = SystemTime::now().duration_since(SystemTime::UNIX_EPOCH).expect("").as_secs() as i64;
        let new_streak = get_lose_streak(self.riot_puuid_id.clone(), self.last_query_time, self.lose_streak).await;
        if new_streak != self.lose_streak {
            self.lose_streak = new_streak;
            return true;
        }
        return false;
    }
}