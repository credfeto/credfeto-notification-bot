CREATE FUNCTION twitch.streamer_insert (
    username_ TEXT,
    started_streaming_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.streamer (
        username,
        started_streaming
        )
    VALUES (
        username_,
        started_streaming_
        );

    RETURN FOUND;
END $$;

ALTER FUNCTION twitch.streamer_insert (
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;
