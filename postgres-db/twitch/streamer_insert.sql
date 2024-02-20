CREATE FUNCTION twitch.streamer_insert (
    username_ TEXT,
    id_ TEXT,
    date_created_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    -- update followers with new username
    UPDATE twitch.stream_follower
    SET follower = username_
    WHERE follower IN (
            SELECT username
            FROM twitch.streamer
            WHERE username <> username_
                AND id = id_
            
            UNION
            
            SELECT username
            FROM twitch.viewer
            WHERE username <> username_
                AND id = id_
            );

    -- update chatters with new username
    UPDATE twitch.stream_chatter
    SET chat_user = username_
    WHERE chat_user IN (
            SELECT username
            FROM twitch.streamer
            WHERE username <> username_
                AND id = id_
            
            UNION
            
            SELECT username
            FROM twitch.viewer
            WHERE username <> username_
                AND id = id_
            );

    -- update streamers with new username
    UPDATE twitch.streamer
    SET username = username_
    WHERE id = id_
        AND username <> username_;

    -- remove any viewer
    DELETE
    FROM twitch.viewer
    WHERE username = username_
        OR id = id_;

    -- add streamer if not exists
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

GRANT EXECUTE
    ON FUNCTION twitch.streamer_insert(TEXT, TEXT, TIMESTAMP WITH TIME zone)
    TO notificationbot;
