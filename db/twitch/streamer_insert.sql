CREATE PROCEDURE twitch.streamer_insert (
    username_ VARCHAR,
    started_streaming_ TIMESTAMP without TIME zone
    ) LANGUAGE sql
AS
$$

INSERT INTO twitch.streamer (
    username,
    started_streaming
    )
VALUES (
    username_,
    started_streaming_
    );$$;

ALTER PROCEDURE twitch.streamer_insert (
    VARCHAR,
    TIMESTAMP
    ) OWNER TO markr;
