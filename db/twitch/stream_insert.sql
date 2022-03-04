create procedure twitch.stream_insert(channel_ varchar(100), start_date_ timestamp)
    language sql
as $$
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

