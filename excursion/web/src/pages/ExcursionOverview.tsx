import headerExcursion from "../assets/img/headerExcursion.png";
import dogsled from "../assets/img/dogsled.png";
import rib from "../assets/img/rib.png";
import whale from "../assets/img/whale.png";
import Carousel from 'react-bootstrap/Carousel';
import { useNavigate } from 'react-router-dom';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min';
import '../style/ExcursionOverview.css'; 
import { useEffect, useState } from "react";

let excursionID = 0;

function ExcursionOverview() {
    const navigate = useNavigate();

    const updateExcursionID = (id: number) => {
        excursionID = id;
        navigate('/whaleSafari');
    };

    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    useEffect(() => {
        const fetchDataForPosts = async () => {
          try {
            const response = await fetch(
              `http://localhost:7072/api/excursions`
            );
            if (!response.ok) {
              throw new Error(`HTTP error: Status ${response.status}`);
            }
            let postsData = await response.json();
            setData(postsData);
            setError(null);
          } catch (err) {
            setData(null);
          } finally {
            setLoading(false);
          }
        };
        fetchDataForPosts();
      }, []);
    if (!data) {
        return <div>Loading...</div>;
    }

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
                    <Carousel.Item interval={10000} onClick={() => updateExcursionID(0)}>
                        <img
                            className="d-block w-100"
                            src={whale}
                            alt="First slide"
                        />
                        <Carousel.Caption>
                            <h3>{data[0].Name}</h3>
                        </Carousel.Caption>
                    </Carousel.Item>
                    <Carousel.Item interval={10000} onClick={() => updateExcursionID(1)}>
                        <img
                            className="d-block w-100"
                            src={dogsled}
                            alt="Second slide"
                        />
                        <Carousel.Caption>
                            <h3>{data[1].Name}</h3>
                        </Carousel.Caption>
                    </Carousel.Item>
                    <Carousel.Item interval={10000} onClick={() => updateExcursionID(2)}>
                        <img
                            className="d-block w-100"
                            src={rib}
                            alt="Third slide"
                        />
                        <Carousel.Caption>
                            <h3>{data[2].Name}</h3>
                        </Carousel.Caption>
                    </Carousel.Item>
                </Carousel>
            </div>
        </div>
    );
};


export { excursionID };
export default ExcursionOverview;