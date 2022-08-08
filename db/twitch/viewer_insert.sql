CREATE FUNCTION twitch.viewer_insert (
    username_ TEXT,
    id_ TEXT,
    date_created_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    -- update chatters with new username
    update twitch.stream_chatter
    set chat_user = username_
    where chat_user in (select username
                        from twitch.viewer
                        where username <> username_
                          and id = id_);

    -- update viewers with new username
    update twitch.viewer
    set username = username_
    where id = id_
      and username <> username_;

    -- add viewer if not exists
    INSERT INTO twitch.viewer (username,
                               id,
                               date_created)
    VALUES (userName_,
            id_,
            date_created_)
    ON conflict do nothing;

    return FOUND;
END $$;

ALTER FUNCTION twitch.viewer_insert (
    TEXT,
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;
