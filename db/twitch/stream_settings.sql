CREATE TABLE twitch.stream_settings
(
    channel             VARCHAR(100) NOT NULL,
    start_date          TIMESTAMP    NOT NULL,
    thanks              BIT          NOT NULL,
    announce_milestones BIT          NOT NULL,
    chat_welcomes       BIT          NOT NULL,
    raid_welcomes       BIT          NOT NULL,
    shout_outs          BIT          NOT NULL,
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
