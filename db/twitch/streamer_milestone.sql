CREATE TABLE twitch.streamer_milestone (
    channel VARCHAR(100) NOT NULL,
    followers INT NOT NULL,
    date_created TIMESTAMP NOT NULL,
    CONSTRAINT streamer_milestone_pk UNIQUE (
        channel,
        followers
        )
    );

ALTER TABLE twitch.streamer_milestone OWNER TO markr;

GRANT INSERT,
    SELECT,
    UPDATE
    ON twitch.streamer_milestone
    TO notificationbot;
