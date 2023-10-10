# [Visit the wiki](https://github.com/Carepatron/Carepatron-Test-Full/wiki)

1. Quality and best practices.
    - I created a separate service to handle sepcial logic before calling IClientRepository.

2. Can this submission's code architecture easily scale to a codebase with 20 developers?
    - I think one of the best way is implementing Single Responsibility Priniciple
    - Proper documentation.
    - Don't over architect (KISS).
3. How can you ensure data integrity in case of failures?
    - Retry mechanisms
    - Logging/Auditing
    - Frequent Backup of data
4. How can you ensure the API behaves as you intend it to?
    - Write Unit/Integration Tests
5. How can you improve the performance of this?
    - For sending email and updating documents, I think it should be a background process, instead of executing it together with the database-related tasks.
6. Feel free to mock any external infrastructure as in-memory if you need.