-- Public election records carried forward from WA1, plus the Richmond slate.
SET IDENTITY_INSERT [Municipalities] ON;
INSERT INTO [Municipalities] ([Id], [Name], [Slug], [Province], [ElectionDate], [Description], [Accent]) VALUES
(1, N'Pitt Meadows', N'pitt-meadows', N'British Columbia', N'2026-10-17', N'Between the Fraser River and the mountains, help shape the future of our close-knit community.', N'#405E42'),
(2, N'Maple Ridge', N'maple-ridge', N'British Columbia', N'2026-10-17', N'A growing community rooted in nature. Meet the people working toward a thriving, connected Maple Ridge.', N'#844B32'),
(3, N'Burnaby', N'burnaby', N'British Columbia', N'2026-10-17', N'Diverse neighbourhoods, shared possibilities. Get to know the voices standing for your community.', N'#1C6179'),
(4, N'Surrey', N'surrey', N'British Columbia', N'2026-10-17', N'A city of possibilities. Explore the candidates shaping one of British Columbia''s fastest-growing communities.', N'#5C5481'),
(5, N'Richmond', N'richmond', N'British Columbia', N'2026-10-17', N'Meet the candidates seeking to represent Richmond’s island neighbourhoods.', N'#965035');
SET IDENTITY_INSERT [Municipalities] OFF;

SET IDENTITY_INSERT [Positions] ON;
INSERT INTO [Positions] ([Id], [MunicipalityId], [Name], [SeatsAvailable]) VALUES
(1, 1, N'Mayor', 1),
(2, 1, N'Councilor', 6),
(3, 1, N'School Trustee', 2),
(4, 2, N'Mayor', 1),
(5, 2, N'Councilor', 6),
(6, 2, N'School Trustee', 2),
(7, 3, N'Mayor', 1),
(8, 3, N'Councilor', 8),
(9, 3, N'School Trustee', 7),
(10, 4, N'Mayor', 1),
(11, 4, N'Councilor', 8),
(12, 4, N'School Trustee', 6),
(13, 5, N'Mayor', 1),
(14, 5, N'Councillor', 8),
(15, 5, N'School Trustee', 7);
SET IDENTITY_INSERT [Positions] OFF;

SET IDENTITY_INSERT [Candidates] ON;
INSERT INTO [Candidates] ([Id], [MunicipalityId], [PositionId], [Name], [Profile], [Tagline], [Website], [Email], [SocialUrl], [VideoUrl], [DateCreated], [PortraitNumber]) VALUES
(1, 1, 1, N'Amelia Chen', N'As a community planner and longtime Pitt Meadows resident, Amelia believes our city''s best days are ahead. Her priorities are thoughtful growth, attainable housing, and safe, welcoming public spaces.

She has spent the last decade working with neighbourhood groups on accessible parks and transportation. As mayor, she would bring residents into the conversation early and publish clear updates on city spending.', N'A connected community. A brighter tomorrow.', NULL, NULL, NULL, NULL, N'2026-08-02T00:00:00', 1),
(2, 1, 1, N'Daniel Brooks', N'Daniel is a local business owner and volunteer who has called Pitt Meadows home for over twenty years. He is focused on responsible budgets, reliable infrastructure, and helping small businesses thrive.

His campaign centres on transparent decision-making and measurable improvements to roads, recreation facilities, and city services.', N'Practical leadership. Lasting progress.', NULL, NULL, NULL, NULL, N'2026-08-03T00:00:00', 2),
(3, 1, 2, N'Aisha Patel', N'Aisha is a housing advocate and nonprofit director. She wants people at every stage of life to find a home in Pitt Meadows. Her priorities include affordable housing, accessible community services, and a stronger voice for young families.

She volunteers with local food programs and supports collaborative planning that brings residents, builders, and local businesses together.', N'Room to grow. A place to belong.', NULL, NULL, NULL, NULL, N'2026-08-04T00:00:00', 3),
(4, 1, 2, N'Benjamin Lee', N'Benjamin is a transportation engineer and active community volunteer. He is running to improve safe routes to school, transit connections, and everyday cycling infrastructure.

He supports practical investments backed by open data, with a focus on maintaining existing infrastructure before expanding it.', N'Moving our community forward, together.', NULL, NULL, NULL, NULL, N'2026-08-05T00:00:00', 4),
(5, 1, 2, N'Clara Wilson', N'Clara is an environmental educator with a passion for our rivers, trails, and farmland. She advocates for climate resilience, responsible land use, and protecting the natural character of Pitt Meadows.

Her approach combines community-led conservation with practical improvements to drainage, tree cover, and emergency preparedness.', N'Protecting the places we call home.', NULL, NULL, NULL, NULL, N'2026-08-06T00:00:00', 5),
(6, 1, 2, N'David Okafor', N'David is an entrepreneur and youth mentor. He wants to create opportunities for local businesses while ensuring neighbourhoods remain affordable and welcoming.

His priorities include neighbourhood gathering spaces, efficient city services, and partnerships that connect young people with meaningful work.', N'Local businesses. Stronger neighbourhoods.', NULL, NULL, NULL, NULL, N'2026-08-07T00:00:00', 6),
(7, 1, 2, N'Elena Martinez', N'Elena is a public health professional who advocates for accessible recreation and healthy, connected neighbourhoods. She supports thoughtful investments in community facilities and services for seniors.

She believes city decisions should be easy to understand and shaped by the people they affect.', N'A city that works for everyone.', NULL, NULL, NULL, NULL, N'2026-08-08T00:00:00', 7),
(8, 1, 2, N'Finn Robertson', N'Finn is a carpenter and longtime volunteer coach. His priorities are safe streets, responsible development, and recreational opportunities that families can afford.

He brings hands-on experience with construction and a commitment to maintaining the city''s parks and public facilities.', N'Thoughtful growth, grounded in community.', NULL, NULL, NULL, NULL, N'2026-08-09T00:00:00', 8),
(9, 1, 2, N'Grace Kim', N'Grace is a librarian and community organizer. She is running to make local government more accessible, support cultural programming, and strengthen neighbourhood connections.

She proposes more evening public meetings and easy-to-read reporting on city projects and budgets.', N'Your voice at the council table.', NULL, NULL, NULL, NULL, N'2026-08-10T00:00:00', 9),
(10, 1, 3, N'Hannah Singh', N'Hannah is an educator and parent focused on inclusive classrooms, student well-being, and transparent school governance. She supports early learning and better access to learning resources.

She would work with families, teachers, and support staff to ensure students have the help they need to succeed.', N'Every student deserves a strong start.', NULL, NULL, NULL, NULL, N'2026-08-11T00:00:00', 10),
(11, 1, 3, N'Isaac Thompson', N'Isaac is a school volunteer and accountant. He wants responsible school budgets that put students first, with clear communication about how resources are allocated.

His priorities include safe learning environments, mental health support, and reliable consultation with parents.', N'Listening to families. Supporting learning.', NULL, NULL, NULL, NULL, N'2026-08-12T00:00:00', 11),
(12, 1, 3, N'Julia Nguyen', N'Julia is a youth worker with experience in after-school programs and career mentoring. She advocates for inclusive education, arts access, and practical pathways after graduation.

She believes every student should feel welcome and see a future for themselves in their community.', N'Opening doors for the next generation.', NULL, NULL, NULL, NULL, N'2026-08-13T00:00:00', 12),
(13, 3, 7, N'Liam Anderson', N'Liam Anderson is a fictional sample candidate for Mayor in Burnaby. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-13T00:00:00', 1),
(14, 3, 8, N'Maya Desai', N'Maya Desai is a fictional sample candidate for Councilor in Burnaby. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-14T00:00:00', 2),
(15, 3, 9, N'Noah Williams', N'Noah Williams is a fictional sample candidate for School Trustee in Burnaby. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-15T00:00:00', 3),
(16, 2, 4, N'Olivia Park', N'Olivia Park is a fictional sample candidate for Mayor in Maple Ridge. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-16T00:00:00', 4),
(17, 2, 5, N'Peter Johnson', N'Peter Johnson is a fictional sample candidate for Councilor in Maple Ridge. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-17T00:00:00', 5),
(18, 2, 6, N'Quinn Taylor', N'Quinn Taylor is a fictional sample candidate for School Trustee in Maple Ridge. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-18T00:00:00', 6),
(19, 4, 10, N'Rose Ahmed', N'Rose Ahmed is a fictional sample candidate for Mayor in Surrey. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-19T00:00:00', 7),
(20, 4, 11, N'Samuel Wright', N'Samuel Wright is a fictional sample candidate for Councilor in Surrey. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-20T00:00:00', 8),
(21, 4, 12, N'Tara Morrison', N'Tara Morrison is a fictional sample candidate for School Trustee in Surrey. Their demonstration profile focuses on accessible public services, thoughtful planning, and transparent local government.

This record is included to demonstrate municipality filtering and administration. Edit the profile in the Admin area to explore the application.', N'A stronger community starts with all of us.', NULL, NULL, NULL, NULL, N'2026-08-21T00:00:00', 9),
(22, 5, 13, N'Morgan Lau', N'Fictional Richmond candidate. Coastal resilience and neighbourhood planning.', N'Coastal resilience and neighbourhood planning.', NULL, NULL, NULL, NULL, N'2026-08-20', 2),
(23, 5, 14, N'Alex Brar', N'Fictional Richmond candidate. Accessible streets and public recreation.', N'Accessible streets and public recreation.', NULL, NULL, NULL, NULL, N'2026-08-20', 3),
(24, 5, 15, N'Jordan Wu', N'Fictional Richmond candidate. Safe classrooms and after-school programs.', N'Safe classrooms and after-school programs.', NULL, NULL, NULL, NULL, N'2026-08-20', 4);
SET IDENTITY_INSERT [Candidates] OFF;

