CREATE FUNCTION twitch.streamer_insert (
    username_ TEXT,
    id_ TEXT,
    date_created_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    update twitch.streamer
    set username = username_
    where id = id_
      and username <> username_;

    delete from twitch.viewer
    where username = username_
       or id = id_;

    INSERT INTO twitch.streamer (username,
                                 id,
                                 date_created)
    VALUES (userName_,
            id_,
            date_created_)
    ON conflict do nothing;

    return FOUND;
END $$;

ALTER FUNCTION twitch.streamer_insert (
    TEXT,
    TEXT,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;
