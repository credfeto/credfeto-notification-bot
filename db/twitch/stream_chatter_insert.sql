create procedure twitch.stream_chatter_insert(
        channel_ varchar(100),
        start_date_ timestamp,
        chat_user_ varchar(100)
)
    language sql
as $$
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
$$;

