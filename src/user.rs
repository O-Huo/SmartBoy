use crate::riot_client::get_riot_id_for_user;
use crate::riot_client::get_lose_streak;
use std::time::SystemTime;
pub(crate) struct User {
    user_name: String,
    riot_id: String,
    nick_name: String,
    last_query_time: i64,
    lose_streak: i32,
}


impl User {
    pub async fn new(user_name: String, nick_name: String) -> Self {
        let riot_id = get_riot_id_for_user(user_name.clone()).await;
        Self {
            user_name,
            lose_streak: get_lose_streak(riot_id.clone(), 0, 0).await,
            riot_id,
            nick_name,
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