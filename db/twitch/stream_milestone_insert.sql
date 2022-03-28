create function twitch.stream_milestone_insert(channel_ character varying, followers_ integer)
    returns TABLE(channel character varying, followers integer, freshly_reached boolean)
    language plpgsql
as
$$
BEGIN
    INSERT INTO twitch.streamer_milestone (
        channel,
        followers,
        date_created
    )
    VALUES (
               channel_,
               followers_,
               now()
           )
    ON conflict do nothing;

    return query(
        select s.channel as channel,
               max(s.followers) as followers,
               found and max(s.followers) = followers_ as freshly_reached
        from twitch.streamer_milestone as s
        where s.channel = channel_
        group by s.channel
            );
END;
$$;

alter function twitch.stream_milestone_insert(varchar, integer) owner to markr;

grant execute on function twitch.stream_milestone_insert(varchar, integer) to notificationbot;

