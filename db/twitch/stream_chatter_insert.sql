CREATE FUNCTION twitch.stream_chatter_insert (
    channel_ VARCHAR,
    start_date_ TIMESTAMP WITH TIME zone,
    chat_user_ VARCHAR,
    chat_id_ VARCHAR
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    update twitch.stream_chatter
    set chat_id = chat_id_
    where chat_id is null
    and chat_user = chat_user_;

    update twitch.stream_chatter
    set chat_user = chat_user_
    where chat_id = chat_id_
      and chat_user <> chat_user;

    INSERT INTO twitch.stream_chatter (
        channel,
        start_date,
        chat_user,
        chat_id,
        first_message_date
        )
    VALUES (
        channel_,
        start_date_,
        chat_user_,
        chat_id_,
        now()
        )
        ON conflict do nothing;

    RETURN found;
END;$$;

ALTER FUNCTION twitch.stream_chatter_insert (
    VARCHAR,
    TIMESTAMP WITH TIME zone,
    VARCHAR
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_chatter_insert(VARCHAR, TIMESTAMP WITH TIME zone, VARCHAR, VARCHAR)
    TO notificationbot;
