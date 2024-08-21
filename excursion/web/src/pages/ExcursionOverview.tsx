import headerExcursion from "../assets/img/headerExcursion.png";
import dogsled from "../assets/img/dogsled.png";
import rib from "../assets/img/rib.png";
import whale from "../assets/img/whale.png";
import Carousel from 'react-bootstrap/Carousel';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min';
import '../style/ExcursionOverview.css'; 

function ExcursionOverview() {
    return (
        <div className="main">
            <img src={headerExcursion} alt="Test bilde2" />
            <div className="header">
                <h1>Excursions</h1>
            </div>
            <div className="middleText">
                <h3>Crafting Arctic Adventures</h3>
                <p>One Thrill at a Time</p>
                <p>Browse through our many different excursions</p>
            </div>
            <div>
                <Carousel>
                    <Carousel.Item interval={10000}>
                        <img
                            className="d-block w-100"
                            src={whale}
                            alt="First slide"
                        />
                        <Carousel.Caption>
                            <h3>Whale Safari</h3>
                        </Carousel.Caption>
                    </Carousel.Item>
                    <Carousel.Item interval={10000}>
                        <img
                            className="d-block w-100"
                            src={dogsled}
                            alt="Second slide"
                        />
                        <Carousel.Caption>
                            <h3>Dog Sled</h3>
                        </Carousel.Caption>
                    </Carousel.Item>
                    <Carousel.Item interval={10000}>
                        <img
                            className="d-block w-100"
                            src={rib}
                            alt="Third slide"
                        />
                        <Carousel.Caption>
                            <h3>Rib</h3>
                        </Carousel.Caption>
                    </Carousel.Item>
                </Carousel>
            </div>
        </div>
    );
}

export default ExcursionOverview;