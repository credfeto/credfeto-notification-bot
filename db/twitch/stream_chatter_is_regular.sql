create function twitch.stream_chatter_is_regular(channel_ character varying, username_ character varying)
    returns TABLE(chat_user character varying, regular boolean)
    language plpgsql
as
$$
BEGIN
    return QUERY (
        select c.chat_user, c.regular
        from (
                 select sc.chat_user, true as regular
                 from twitch.stream_chatter sc
                 where sc.channel = channel_
                   and sc.chat_user = username_
                 group by sc.chat_user
                 having count(*) > 2
                 union
                 select username_ as chat_user,
                        false     as regular
             ) as c
        order by c.regular desc
        limit 1
    );

END;
$$;

alter function twitch.stream_chatter_is_regular(varchar, varchar) owner to markr;

