use crate::user::User;

trait Store {
    fn user_list(&self) -> Vec<User>;

    fn add_or_update_user(&self, user: User);
}

struct StoreImpl {
}

impl StoreImpl {

}