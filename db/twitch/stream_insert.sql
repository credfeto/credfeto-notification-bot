create procedure twitch.stream_insert(channel_ character varying, start_date_ timestamp without time zone)
    language sql
as
$$
insert into twitch.stream
(
    channel,
    start_date
)
values
    (
        channel_,
        start_date_
    );
$$;

alter procedure twitch.stream_insert(varchar, timestamp) owner to markr;

