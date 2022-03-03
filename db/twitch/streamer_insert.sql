CREATE PROCEDURE twitch.streamer_insert (
    username_ VARCHAR,
    started_streaming_ DATE
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.streamer (
        username,
        started_streaming
        )
    VALUES (
        username_,
        started_streaming
        );
END $$;
