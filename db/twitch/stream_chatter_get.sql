create function twitch.stream_chatter_get(channel_ character varying, start_date_ timestamp with time zone, chat_user_ character varying)
    returns TABLE(channel character varying, start_date timestamp without time zone, chat_user character varying, first_message_date timestamp without time zone)
    language plpgsql
as
$$
begin
    return QUERY(
        select
            s.channel,
            s.start_date,
            s.chat_user,
            s.first_message_date
        from twitch.stream_chatter s
        where s.channel = channel_
        and s.start_date = start_date_
        and s.chat_user = chat_user_
    );
end;
$$;

alter function twitch.stream_chatter_get(varchar, timestamp with time zone, varchar) owner to markr;

