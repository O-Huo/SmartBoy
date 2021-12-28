mod riot_client;
mod store;
mod telegram_client;
mod user;
use frankenstein::Api;
use frankenstein::SetWebhookParams;
use frankenstein::Update;
use frankenstein::TelegramApi;
use std::env;
use std::net::Ipv4Addr;
use tokio;
use warp::{http, Filter};

async fn process_telegram_updates(
    updates: Vec<Update>
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

#[tokio::main]
async fn main() {
    let token = "";
    let api = Api::new(token);

    let webhook_params = SetWebhookParams {
        url: String::from("123"),
        certificate: None,
        ip_address: None,
        max_connections: None,
        allowed_updates: None,
        drop_pending_updates: None,
    };
    api.set_webhook(&webhook_params).expect("Set webhook failed");

    let tg_handler = warp::post()
    .and(warp::path("api"))
    .and(warp::path("tg-update"))
    .and(warp::path::end())
    .and(warp::body::json())
    .and_then(process_telegram_updates);

    let port = 3000;

    warp::serve(tg_handler)
        .run((Ipv4Addr::UNSPECIFIED, port))
        .await
}
