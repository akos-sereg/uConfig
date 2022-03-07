create table device (
	id varchar(40) primary key, 
    name varchar(32),
    platform varchar(32),
    user_id int
);


create table device_config_item (
	device_id varchar(40),
    config_key varchar(15),
    config_value varchar(64)
);

create table device_activity(
	device_id varchar(40),
    endpoint varchar(64),
    is_success int(11),
    error_message varchar(128),
    inserted_at datetime
);

create table device_log(
	device_id varchar(40),
    log_message text,
    inserted_at datetime
);

create table uconfy_user (
	user_id int,
    email varchar(32),
    password varchar(64),
    api_key varchar(64),
    registered_at datetime
)

ALTER TABLE uconfy_user MODIFY COLUMN user_id INT primary key auto_increment