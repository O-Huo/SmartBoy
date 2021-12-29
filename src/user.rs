use crate::riot_client::get_riot_id_for_user;
use crate::riot_client::get_lose_streak;
use std::time::SystemTime;
use serde::Deserialize;
use serde::Serialize;
use std::hash::Hash;

#[derive(Serialize, Deserialize, Clone)]
pub struct User {
    pub user_name: String,
    pub riot_id: String,
    pub tg_id: u64,
    pub tg_name: String,
    pub last_query_time: i64,
    pub lose_streak: i32,
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
        let riot_id = get_riot_id_for_user(user_name.clone()).await;
        Self {
            user_name,
            lose_streak: get_lose_streak(riot_id.clone(), 0, 0).await,
            riot_id,
            tg_id,
            tg_name,
            last_query_time: SystemTime::now().duration_since(SystemTime::UNIX_EPOCH).expect("").as_secs() as i64,
        }
    }

    pub async fn update(& mut self) -> bool {
        let new_streak = get_lose_streak(self.riot_id.clone(), self.last_query_time, self.lose_streak).await;
        if new_streak != self.lose_streak {
            self.lose_streak = new_streak;
            return true;
        }
        return false;
    }
}