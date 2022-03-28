create function twitch.stream_chatter_insert(channel_ character varying, start_date_ timestamp with time zone, chat_user_ character varying) returns boolean
    language plpgsql
as
$$
BEGIN
    INSERT INTO twitch.stream_chatter (
        channel,
        start_date,
        chat_user,
        first_message_date
    )
    VALUES (
               channel_,
               start_date_,
               chat_user_,
               now()
           )
    ON conflict do nothing;

    RETURN found;
END;
$$;

alter function twitch.stream_chatter_insert(varchar, timestamp with time zone, varchar) owner to markr;

grant execute on function twitch.stream_chatter_insert(varchar, timestamp with time zone, varchar) to notificationbot;

