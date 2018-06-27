# Blue Ribbon Review Live Project Stories
Below are five stories I completed while working on the "Blue Ribbon Review" Live Project Team. The stories vary from front-end styling changes to back-end work. I have included relevant code snippets and images below, but I have also added all the files I worked on in this repository. Feel free to look further into any of the files for a broader scope.

### 1. **Created partial view to render on site's Landing page, with a carousel that loops through images of campaigns (product discounts) that are within 7 days of their expiration date**
Relevant Files:
- `CampaignsController.cs`
- `Landing.cshtml`
- `_LandingPartial.cshtml`

My time on the project came to an end before I was able to implement the functionality of the carousel. However, I was able to create a partial view, `_LandingPartial.cshtml`, with the basic Bootstrap carousel modal and connect the logic I wrote in the `CampaignsController.cs` to the front end.

Below is a screenshot of the `GetLandingPartial()` method I created in the `CampaignsController.cs`. I created a variable, `expiringCampagins`, containing any campaigns with an expire date within seven days of expiring, which is then returned to the `_LandingPartial.cshtml`.
![](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/Carousel/CampControllerCode.JPG)

The `GetLandingPartial()` method from the `CampaignsController.cs` is then called by a `Html.RenderAction` on the `Landing.cshtml` (see code & Landing page screenshots below). As stated earlier, I did not have time to write the JavaScript to make the carousel function with images, but the `Html.RenderAction` successfully rendered the blank carousel modal from the `_LandingPartial.cshtml` by using the `GetLandingPartial()` method.
![Html.RenderAction Code](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/Carousel/LandingPageDivCode.JPG)

![Landing Page](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/Carousel/LandingPageCarousel.JPG)

### 2. **Ensured that campaign products do not display on the Reviews->Index page more than once**
Relevant Files:
- `ReviewsController.cs`
- Reviews -> `Index.cshtml`

We were having an issue on the Index page for our reviews, where our accordion modal displaying products was creating duplicates of the same unique product. Users were meant to see one of each product that had one or more reviews and could click on the product heading to drop down an accordion list of every review for that product.

In order to solve this redundancy, I created a new variable `uniqueCampaignsWithReviews` in `Index()` method of the `ReviewsController.cs`, which contains only unique instances of campaigns with one or more reviews (see `uniqueCampaignsWithReviews` variable in code block from `ReviewsController.cs` below).

![Reviews Controller Code](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/ReviewsRedundency/ReviewsControllerCode.JPG)

Screenshot of corrected accordion modal with only one instance of unique products:
![Reviews -> Index.cshtml](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/ReviewsRedundency/ReviewsIndexExmple.JPG)

### 3. **On the \_Details partial of the Campaigns->Index view, create a hover effect that displays "Please log in!" when a user who is not logged in hovers over the "Review this Product" link**
Relevant Files:
- `_Details.cshtml`
- Campagins -> `Index.cshtml`

I created a if-else statement on the `_Details` partial view to make sure only users that are logged in with an account can access the link to create a review. Also added a tooltip hover over message for users not logged in.

See code on the `_Details` partial and screenshot of the hover effect below:

![`_Details` Partial Code](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/ReviewHoverEffect/_DetailsPartialCode.JPG)
![Hover Effect Screenshot](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/ReviewHoverEffect/ToolTipMessage.png)

### 4. **Made the product image on the table in the Campaigns->AdminView a link to the Campaigns->Details page for that product**
Relevant Files:
- `AdminView.cshtml`

To resolve, I created a `Url.Action()` link for images in the AdminView table. See the code on the `AdminView.cshtml` file and a screenshot of the image link:
![`AdminView.cshtml` Code Snippit](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/ImageLink/ImageLinkCode.JPG)
![`AdminView` Page View](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/ImageLink/ImageLink.png)

### 5. **Changed user email links on the Admin->Index page to font awesome paper plane icons**
Relevant Files:
- Admin -> `Index`

Originally the text of the user emails in the table on the Admin->`Index` page was a link to send an email. I removed the text link and moved it to a paper plane font awesome icon at the end of each user email in the table.

See the code on the Admin->`Index` page below, as well as a screenshot of the Index page:
![Admin->`Index` Code](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/PaperPlaneEmailLink/EmailCode.JPG)
![Admin->`Index` Paper Plane Links](https://github.com/duncanhm/BlueRibbonsReview-LiveProject-Stories/blob/master/LP_ScreenShots/PaperPlaneEmailLink/EmailHover.png)
