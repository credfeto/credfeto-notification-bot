CREATE TABLE twitch.stream_follower (
    channel VARCHAR(100) NOT NULL,
    follower VARCHAR(100) NOT NULL,
    follow_count INT NOT NULL,
    first_followed TIMESTAMP NOT NULL,
    last_followed TIMESTAMP NOT NULL,
    CONSTRAINT stream_follower_pk PRIMARY KEY (
        channel,
        follower
        )
    );

ALTER TABLE twitch.stream_follower OWNER TO markr;

GRANT INSERT,
    SELECT,
    UPDATE
    ON twitch.stream_follower
    TO notificationbot;
