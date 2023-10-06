# HW5 TikTok Wall

Создать ленту тикток, с отображением актуальной информации:
* Видео на весь экран
* Внизу никнейм или сигнатура
* Ниже время создания
* Ниже первая строчка описания, при нажатии описание разворачивается
* Ниже название звука. Оно прокручивается справа налево
* Внизу справа табнейл звука
* Справа посеридине аватарка автора. При нажатии открывается страница автора с видео
* Ниже значок просмотра и под ним количество просмотров с округлением до тысяч, миллионов, миллиардов
* Ниже иконка комментариев и под ним их количество
* Ниже кнопка поделиться. При нажатии шарится видео, а не ссылка на него

Для подгрузки json использовать прокси  

* Ссылка для json ленты https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/  
* Ссылка для json странички https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/aweme/post/?user_id={uid}&count=21&max_cursor={max_cursor}&aid=1128  
Для первых видео max_cursor == 0. Для подзагрузки следующих будет указан в конце первого запроса  
* Ссылка для json комментариев https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/comment/list/?aweme_id={aweme_id}&count=20&cursor={cursor}  
Для первых комментариев cursor == 0. Для подзагрузки следующих будет указан в конце первого запроса  
* Ссылка для json реплаев https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/comment/list/reply/?comment_id={cid}&cursor=0&count=20  
Для первых комментариев cursor == 0. Для подзагрузки следующих будет указан в конце первого запроса  

После подгрузки ленты, необходимо делать замер как долго юзер смотрел те или иные видео и переводить значение в проценты. Условно если просмотрено 10 секунд из 100, то это 10%. Далее собирается рейтинг по формуле {просмотренный процент} * 70% + {стоит лайк ? 1 : 0} * 30%. По самому большому показателю выбирается aweme_id и по нему подгружается следующая пачка видео

Ссылка для json ленты с рекомендацией по видео https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/?aweme_id={aweme_id}

Home feeds
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/

someone's videos
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/aweme/post?user_id=83774364341&max_cursor=0&count=20/

someone's likes
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/aweme/favorite?user_id=83774364341&max_cursor=0&count=20/

someone's info
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/user?user_id=83774364341

someone's followings
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/user/following/list?user_id=83774364341&max_time=1541202996

someone's followers
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/user/follower/list?user_id=83774364341&max_time=1541473941

video's coments
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/comment/list?aweme_id=6615981222587796743&cursor=0

hot topics
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/category/list?cursor=0/

topics list
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/challenge/aweme?ch_id=1617915729448973&cursor=0

topic info
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/challenge/detail?ch_id=1617915729448973

search users
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/search/discover?keyword=lucas&cursor=0

search musics
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/search/music?keyword=lucas&cursor=0

search topics
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/search/challenge?keyword=lucas&cursor=0

search videos
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/search/item?keyword=lucas&cursor=0

goods's list
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/user/promotions?user_id=93712507975&cursor=0

live videos list
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/room/feed?cursor=0

hot topics
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/hotsearch/word

hot videos
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/hotsearch/aweme

hot energy
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/hotsearch/energy

https://api.tiktokv.com/aweme/v1/playwm/?video_id=$url
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/?aweme_id=6930858277987241217
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/comment/list/?aweme_id=6930858277987241217&count=20&cursor=20
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/aweme/post/?user_id=6826352087568581637&count=21&min_cursor=1559466150000&aid=1128&_signature=0jFsbRAdiYt4Hiztxpb839IxbH&dytk=e56d7c22dbb1d0bb0cab80cce0d96e17
https://api-t2.tiktokv.com/aweme/v1/aweme/post/?user_id=6913968433924326401
https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/user/profile/other/?user_id=6868565347890299906&ac=wifi&channel=googleplay&aid=1233&app_name=musical_ly&version_code=230703&version_name=23.7.3&device_platform=android&ab_version=23.7.3&ssmix=a&device_type=POT-LX17&device_brand=HUAWEI&device_model=POT-LX17&language=en&os_api=29&os_version=10&manifest_version_code=2022307030&resolution=1080%2A2340&dpi=480&update_version_code=2022307030&app_skin=white&app_type=normal&sys_region=US&pass-route=1&mcc_mnc=31050&pass-region=1&timezone_name=America%252FNew_York&carrier_region_v2=310&cpu_support64=true&host_abi=arm64-v8a&app_language=en&carrier_region=US&ac2=wifi&uoo=0&op_region=US&timezone_offset=-14400&build_number=23.7.3&locale=en&region=US&openudid=bab65b3c159a4542&cdid=1856ed1e-b818-44b1-8bf1-136ff95e429a&device_id=7187122281045952042&iid=7187123175168165678&_rticket=1674146527532&ts=1674146588