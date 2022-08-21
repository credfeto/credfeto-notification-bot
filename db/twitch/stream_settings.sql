CREATE TABLE twitch.stream_settings
(
    channel             VARCHAR(100) NOT NULL,
    start_date          TIMESTAMP    NOT NULL,
    thanks              boolean      NOT NULL,
    announce_milestones boolean      NOT NULL,
    chat_welcomes       boolean      NOT NULL,
    raid_welcomes       boolean      NOT NULL,
    shout_outs          boolean      NOT NULL,
    CONSTRAINT stream_settings_pk PRIMARY KEY (
                                               channel,
                                               start_date
        )
);

ALTER TABLE twitch.stream_settings
    OWNER TO markr;

GRANT DELETE,
    INSERT,
    SELECT,
    UPDATE
    ON twitch.stream_settings
    TO notificationbot;
