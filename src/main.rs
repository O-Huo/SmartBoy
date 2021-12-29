mod riot_client;
mod store;
mod telegram_client;
mod user;
use crate::store::FileStore;
use crate::user::User;
use frankenstein::Api;
use frankenstein::ChatType;
use frankenstein::GetUpdatesParamsBuilder;
use frankenstein::SendMessageParamsBuilder;
use frankenstein::TelegramApi;
use frankenstein::Update;
use std::env;
use std::os::unix::prelude::CommandExt;
use std::time::SystemTime;
use tokio;
use tokio::time::{sleep, Duration};
use warp::{http, Filter};

async fn process_telegram_updates(
    updates: Vec<Update>,
) -> Result<impl warp::Reply, warp::Rejection> {
    println!("updates received");
    for update in updates {
        // println!(update.message.)
    }
    Ok(warp::reply::with_status(
        "Added items to the grocery list",
        http::StatusCode::CREATED,
    ))
}

fn build_message(users: Vec<User>) -> String {
    let mut users_clone = users.clone();
    users_clone.sort_by(|a, b| b.lose_streak.partial_cmp(&a.lose_streak).unwrap());
    let mut user_string = users_clone.iter().fold(String::new(), |acc, user| {
        acc + &format!("{}: {}", user.tg_name, user.lose_streak) + "\n"
    });
    user_string.pop();
    return "莲贵榜:\n".to_owned() + &user_string;
}

fn send_message(store: &mut FileStore, api: &Api) {
    let text = build_message(store.user_list());
    for group in &store.store.joined_group {
        let send_message_params = SendMessageParamsBuilder::default()
            .chat_id(group.clone())
            .text(&text)
            .build()
            .unwrap();
        if let Err(err) = api.send_message(&send_message_params) {
            println!("Failed to send message: {:?}", err);
        }
    }
}

#[tokio::main]
async fn main() {
    let token = env::var("TG_KEY").unwrap();
    let api = Api::new(&token);

    let mut update_params_builder = GetUpdatesParamsBuilder::default();
    update_params_builder.allowed_updates(vec!["message".to_string()]);

    let mut update_params = update_params_builder.build().unwrap();
    let mut store = FileStore::new(String::from("./out.txt"));
    let mut last_update_time = SystemTime::now();

    loop {
        let result = api.get_updates(&update_params);

        println!("result: {:?}", result);

        match result {
            Ok(response) => {
                for update in response.result {
                    if let Some(message) = update.message {
                        if let Some(text) = message.text {
                            if text.starts_with("/add") {
                                let strs: Vec<&str> = text.split(" ").collect();
                                if strs.len() > 1 {
                                    let user_id = strs[1];
                                    let from = message.from.unwrap();
                                    let tg_id = from.id;
                                    let tg_name = from.first_name;
                                    store.add_or_update_user(
                                        tg_id,
                                        User::new((&user_id).to_string(), tg_id, tg_name).await,
                                    );
                                    if message.chat.type_field == ChatType::Supergroup
                                        || message.chat.type_field == ChatType::Group
                                    {
                                        store.add_group(message.chat.id)
                                    }
                                    send_message(&mut store, &api);
                                }
                            }
                        }
                        update_params = update_params_builder
                            .offset(update.update_id + 1)
                            .build()
                            .unwrap();
                    }
                }
            }
            Err(error) => {
                println!("Failed to get updates: {:?}", error);
            }
        }
        // if SystemTime::now()
            // .duration_since(last_update_time)
            // .unwrap()
            // .as_secs()
            // > 30
        // {
            // if store.check_update() {
                // send_message(&mut store, &api);
            // }
            // last_update_time = SystemTime::now();
        // }
    }
}
