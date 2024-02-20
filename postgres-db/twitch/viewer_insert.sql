CREATE FUNCTION twitch.viewer_insert (
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
            FROM twitch.viewer
            WHERE username <> username_
                AND id = id_
            );

    -- update chatters with new username
    UPDATE twitch.stream_chatter
    SET chat_user = username_
    WHERE chat_user IN (
            SELECT username
            FROM twitch.viewer
            WHERE username <> username_
                AND id = id_
            );

    -- update viewers with new username
    UPDATE twitch.viewer
    SET username = username_
    WHERE id = id_
        AND username <> username_;

    -- add viewer if not exists
    INSERT INTO twitch.viewer (
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

ALTER FUNCTION twitch.viewer_insert (
    TEXT,
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.viewer_insert(TEXT, TEXT, TIMESTAMP WITH TIME zone)
    TO notificationbot;
