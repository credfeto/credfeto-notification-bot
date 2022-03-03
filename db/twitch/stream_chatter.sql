CREATE TABLE twitch.stream_chatter (
    channel VARCHAR(100),
    start_date TIMESTAMP,
    chat_user VARCHAR(100),
    first_message_date TIMESTAMP,
    CONSTRAINT stream_chatter_pk PRIMARY KEY (
        channel,
        start_date,
        chat_user
        )
    );
