CREATE FUNCTION twitch.streamer_insert (
    username_ TEXT,
    id_ TEXT,
    date_created_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.streamer (
        username,
        id,
        date_created
        )
    VALUES (
        userName_,
        id_,
        date_created_
        )
        ON conflict do nothing;

    RETURN FOUND;
END $$;

ALTER FUNCTION twitch.streamer_insert (
    TEXT,
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;
