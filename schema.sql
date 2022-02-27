CREATE USER 'root'@'%' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' WITH GRANT OPTION;
FLUSH PRIVILEGES;

create table device (
	id varchar(40) primary key, 
    name varchar(32),
    platform varchar(32),
    user_id int
)