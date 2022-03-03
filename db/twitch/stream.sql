CREATE TABLE twitch.stream (
    channel VARCHAR(100) NOT NULL,
    start_date TIMESTAMP,
    CONSTRAINT stream_pk PRIMARY KEY (
        channel,
        start_date
        )
    );
