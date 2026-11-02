-- Run this script in MySQL Workbench on the Mac that hosts MySQL Server.
-- Replace the sample password below with your own strong password.

CREATE DATABASE IF NOT EXISTS elevator_db;
USE elevator_db;

CREATE TABLE IF NOT EXISTS elevator_log (
    id INT NOT NULL AUTO_INCREMENT,
    event_time DATETIME NOT NULL,
    from_floor INT NOT NULL,
    to_floor INT NOT NULL,
    description VARCHAR(255) NOT NULL,
    PRIMARY KEY (id)
);

-- A dedicated application account is safer than connecting the app as root.
CREATE USER IF NOT EXISTS 'elevator_app'@'%' IDENTIFIED BY 'CHANGE_THIS_PASSWORD';
ALTER USER 'elevator_app'@'%' IDENTIFIED BY 'CHANGE_THIS_PASSWORD';
GRANT SELECT, INSERT ON elevator_db.elevator_log TO 'elevator_app'@'%';
FLUSH PRIVILEGES;

SELECT * FROM elevator_log ORDER BY id DESC LIMIT 100;
