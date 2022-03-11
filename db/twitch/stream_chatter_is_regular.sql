create function twitch.stream_chatter_is_regular(channel_ character varying, username_ character varying)
    returns TABLE(chat_user character varying, regular boolean)
    language plpgsql
as
$$
BEGIN
    return query (
        select username_ as chat_user,
               exists(
                       select chat_user
                       from twitch.stream_chatter
                       where channel = channel_
                         and chat_user = username_
                       group by chat_user
                       having count(*) > 2
                   )       as regular
    );
END;
$$;

alter function twitch.stream_chatter_is_regular(varchar, varchar) owner to markr;

