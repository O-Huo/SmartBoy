use crate::user::User;
use futures::executor::block_on;
use std::collections::HashMap;
use std::fs;
use std::path::Path;
use serde::Deserialize;
use serde::Serialize;
use std::collections::HashSet;
use std::time::Duration;
use futures::join;
use tokio::time::timeout;

#[derive(Serialize, Deserialize, Clone)]
pub struct Store {
    pub user_list: HashMap<u64, User>,
    pub joined_group: HashSet<i64>,
}

pub struct FileStore {
    pub store: Store,
    path: String,
}

impl FileStore {
    pub fn new(path: String) -> Self {
        let store: Store = if Path::new(&path).exists() {
            serde_json::from_str(&fs::read_to_string(&path).unwrap()).unwrap()
        } else {
            Store {
                user_list: HashMap::new(),
                joined_group: HashSet::new()
            }
        };
        Self { store, path }
    }

    fn persist(&self) {
        fs::write(&self.path, serde_json::to_string(&self.store).unwrap());
    }

    pub fn user_list(&self) -> Vec<User> {
        return self.store.user_list.values().cloned().collect();
    }

    pub fn add_or_update_user(&mut self, tg_id: u64, user: User) {
        self.store.user_list.insert(tg_id, user);
        self.persist();
    }

    pub fn add_group(&mut self, group_id: i64) {
        self.store.joined_group.insert(group_id);
        self.persist()
    }

    pub fn refresh(&mut self) {
        for (_, user) in &mut self.store.user_list {
            block_on(user.refresh())
        }
        self.persist();
    }

    pub fn check_update(&mut self) -> bool {
        println!("check update");
        let mut result = false;
        for (_, user) in &mut self.store.user_list {
            let updated = block_on(user.update());
            result |= updated;
        }
        self.persist();
        println!("check finished");
        return result;
    }
}
