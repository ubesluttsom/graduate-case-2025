import { useEffect, useState } from 'react';
import orca from "../assets/img/Orca.png"
import excursion from "../assets/dummyData/excursion.json"

const ButtonDayStyling = {
    backgroundColor: '#091E3B',
    color: 'white',
    border: 'solid',
    padding: '0px 0px',
    margin: '0px',
    font: 'Roboto',
    cursor: 'pointer',
    borderRadius: '5px',
    fontSize: '18px',
    width: '108px',
    height: '48px'
}

const ButtonStyling = {
    backgroundColor: '#FFB46D',
    color: 'black',
    border: 'solid',
    padding: '10px 20px',
    margin: '5px',
    cursor: 'pointer',
    borderRadius: '5px',
    fontSize: '16px',
    width: '175px',
}

const SingleButtonStyling = {
    backgroundColor: '#FFB46D',
    color: 'black',
    border: 'solid',
    padding: '10px 20px',
    margin: '5px',
    cursor: 'pointer',
    borderRadius: '5px',
    fontSize: '16px',
    width: '370px',
}

const DivDayStyling = {
    display: 'flex',
    flexDirection: 'row' as 'row',
    justifyContent: 'space-evenly'
}

const DivDoubleStyling = {
    display: 'flex',
    flexDirection: 'row' as 'row',
    justifyContent: 'space-evenly'
}

const DivSingleStyling = {
    display: 'flex',
    flexDirection: 'row' as 'row',
    justifyContent: 'space-evenly'
}

const WhaleSafari = () => {
    const title = excursion.title
    
    return (
      <div style={{backgroundColor: '#091E3B', height: '932px'}}>
        <img src={orca} />
        <h1 style={{textAlign: 'center', color: '#FFB46D', fontSize: '36px', font:'Roboto Mono'}}>{title}</h1>
        <p style={{textAlign: 'center', color: 'white', fontSize: '18px'}}><i>See the great whales of the artic up close along our Whale Safari.</i><br/><br/></p>
        <p style={{textAlign: 'center', color: 'white', fontSize: '18px'}}><b>Choose your preferred day for a Whale Safari Excursion: </b></p>

        <div style={DivDayStyling}><button style={ButtonDayStyling}>Monday</button><button style={ButtonDayStyling}>Tuesday</button><button style={ButtonDayStyling}>Wednesday</button></div>
        <div style={DivDayStyling}><button style={ButtonDayStyling}>Thursday</button><button style={ButtonDayStyling}>Friday</button><button style={ButtonDayStyling}>Saturday</button></div>
        <div style={DivDoubleStyling}><button style={ButtonStyling}>Info</button><button style={ButtonStyling}>Package list</button></div>
        <div style={DivSingleStyling}><button style={SingleButtonStyling}>Back to Front Page</button></div>
      </div>
    );
  };
  
  export default WhaleSafari;