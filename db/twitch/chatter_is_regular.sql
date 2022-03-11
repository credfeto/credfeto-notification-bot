create function twitch.chatter_is_regular(channel_ character varying, username_ character varying) returns bit
    language plpgsql
as
$$
BEGIN
    select exists(
                   select chat_user
                   from twitch.stream_chatter
                   where channel = channel_
                     and chat_user = username_
                   group by chat_user
                   having count(*) > 2
               );
END;
$$;

alter function twitch.chatter_is_regular(varchar, varchar) owner to markr;

