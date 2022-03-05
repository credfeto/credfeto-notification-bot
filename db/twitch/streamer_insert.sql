CREATE FUNCTION twitch.streamer_insert (
    username_ TEXT,
    datecreated_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.streamer (
        username,
        datecreated
        )
    VALUES (
        userName_,
        dateCreated_
        );

    RETURN FOUND;
END $$;

ALTER FUNCTION twitch.streamer_insert (
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;
