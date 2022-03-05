CREATE PROCEDURE twitch.streamer_insert (
    username_ TEXT,
    started_streaming_ TIMESTAMP WITH TIME zone
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
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;
