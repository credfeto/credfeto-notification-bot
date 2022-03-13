create table twitch.streamer_milestone
(
    channel      varchar(100) not null,
    followers    integer      not null,
    date_created timestamp    not null,
    constraint streamer_milestone_pk
        unique (channel, followers)
);

alter table twitch.streamer_milestone
    owner to markr;

