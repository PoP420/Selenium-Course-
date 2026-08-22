Feature: Contact form

  Scenario: Successful contact form submission
    Given I am on the contact page
    When I fill the contact form with valid data
    And I submit the form
    Then I should see "Thanks for your message! We will contact you shortly."

  Scenario: Missing fields show validation
    Given I am on the contact page
    When I submit the form without filling it
    Then I should see a validation message
