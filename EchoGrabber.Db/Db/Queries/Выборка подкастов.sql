SELECT Podcasts.Title as 'Название подкаста', count(issues.PodcastID) as "Количество выпусков", sum(issues.Size) as 'Общий объём', sum(Issues.Duration) as 'Длительность'
FROM Podcasts
join Issues on Issues.PodcastId=Podcasts.Id
group by Issues.PodcastId
