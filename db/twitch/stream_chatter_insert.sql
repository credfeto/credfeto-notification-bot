create function twitch.stream_chatter_insert(channel_ character varying, start_date_ timestamp without time zone, chat_user_ character varying) returns boolean
    language plpgsql
as
$$
begin
    insert into twitch.stream_chatter
    (
        channel,
        start_date,
        chat_user,
        first_message_date
    )
    values
        (
            channel_,
            start_date_,
            chat_user_,
            now()
        );

    return found;
end;
$$;

alter function twitch.stream_chatter_insert(varchar, timestamp, varchar) owner to markr;

