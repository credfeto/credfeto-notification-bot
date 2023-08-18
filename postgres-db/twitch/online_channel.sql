CREATE TABLE twitch.online_channel
(
    username     VARCHAR(100) NOT NULL
        CONSTRAINT online_channel_pk PRIMARY KEY,
    date_created TIMESTAMP    NOT NULL,
    id           VARCHAR(100) NOT NULL
);

ALTER TABLE twitch.online_channel
    OWNER TO markr;

GRANT DELETE,
    INSERT,
    SELECT,
    UPDATE
    ON twitch.online_channel
    TO notificationbot;
