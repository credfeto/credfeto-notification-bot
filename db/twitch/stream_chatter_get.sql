create function twitch.stream_chatter_get(channel_ character varying, start_date_ timestamp with time zone, chat_user_ character varying)
    returns TABLE(channel character varying, start_date timestamp without time zone, chat_user character varying, first_message_date timestamp without time zone)
    language plpgsql
as
$$
BEGIN
    RETURN QUERY(SELECT s.channel, s.start_date, s.chat_user, s.first_message_date FROM twitch.stream_chatter s WHERE s.channel = channel_
                                                                                                                  AND s.start_date = start_date_
                                                                                                                  AND s.chat_user = chat_user_);
END;
$$;

alter function twitch.stream_chatter_get(varchar, timestamp with time zone, varchar) owner to markr;

grant execute on function twitch.stream_chatter_get(varchar, timestamp with time zone, varchar) to notificationbot;

