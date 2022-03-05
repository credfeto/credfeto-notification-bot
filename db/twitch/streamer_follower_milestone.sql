create table twitch.streamer_follower_milestone
(
    channel_name      varchar(100) not null,
    followers_reached integer      not null,
    when_reached      timestamp    not null,
    constraint streamer_follower_milestone_pk
        primary key (channel_name, followers_reached)
);

alter table twitch.streamer_follower_milestone
    owner to markr;

