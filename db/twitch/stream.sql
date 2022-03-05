CREATE TABLE twitch.stream (
    channel VARCHAR(100) NOT NULL,
    start_date TIMESTAMP NOT NULL,
    CONSTRAINT stream_pk PRIMARY KEY (
        channel,
        start_date
        )
    );

ALTER TABLE twitch.stream OWNER TO markr;
