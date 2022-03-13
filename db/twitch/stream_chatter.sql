CREATE TABLE twitch.stream_chatter (
    channel VARCHAR(100) NOT NULL,
    start_date TIMESTAMP NOT NULL,
    chat_user VARCHAR(100) NOT NULL,
    first_message_date TIMESTAMP NOT NULL,
    CONSTRAINT stream_chatter_pk PRIMARY KEY (
        channel,
        start_date,
        chat_user
        )
    );

ALTER TABLE twitch.stream_chatter OWNER TO markr;

GRANT DELETE,
    INSERT,
    SELECT,
    UPDATE
    ON twitch.stream_chatter
    TO notificationbot;
