CREATE TABLE twitch.streamer (
    username VARCHAR(100) NOT NULL CONSTRAINT streamer_pk PRIMARY KEY,
    started_streaming TIMESTAMP NOT NULL
    );
