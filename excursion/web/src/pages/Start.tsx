import {
  useAccount,
  useMsal,
} from '@azure/msal-react';
import {
  Box,
  Button,
  ButtonGroup,
  Flex,
  Heading,
  Text,
  Tooltip,
} from '@chakra-ui/react';
import { Guest, Room } from 'cms-types';
import { useEffect } from 'react';
import useAccessToken from '../auth/useAccessToken';
import { useGet } from '../hooks/useGet';
import usePost from '../hooks/usePost';
import {Link} from "react-router-dom";
import mountain from "../assets/img/mountain.png"
import startExcursions from "../assets/img/startExcursions.png"
import startRestaurant from "../assets/img/startRestaurant.png"
import startShop from "../assets/img/startShop.png"
import startSpa from "../assets/img/startSpa.png"
import "../style/Start.css"

const Start = () => {
  const { accounts } = useMsal();
  const account = useAccount(accounts[0] || {});
  const accessToken = useAccessToken();
  
  const {
    data: guest,
    isLoading,
    isError,
    mutate,
  } = useGet<Guest>(`/guests/${account?.localAccountId}`);

  const { 
    data: room,
  } = useGet<Room>(`/rooms/${guest?.roomId}`, guest?.roomId !== undefined && guest?.roomId !== '');

  const post = usePost();

  useEffect(() => {
    if (isLoading) return;
    
    const checkAndCreateGuest = async () => {
      if (
        !isLoading &&
        guest?.id === '00000000-0000-0000-0000-000000000000' &&
        account
      ) {
        const newGuestData = {
          firstName: account?.name?.split(' ').slice(0, -1).join(' '),
          lastName: account?.name?.split(' ').slice(-1).join(' '),
          id: account?.localAccountId,
          email: account?.username
        };

        const newGuest = await post('/guests', newGuestData)
          .then(response => {
            if (!response.ok) {
              return { id: '', firstName: '', lastName: '' };
            }
            return response.json();
          })
          .catch(e => {
            console.log(e);
            return { id: '', firstName: '', lastName: '' };
          });

        mutate(newGuest, false);
      }
    };

    checkAndCreateGuest();
  }, [isLoading, guest, account, mutate, post, isError]);

  return (
    <Flex
      width="100vw"
      height="100vh"
      alignContent="center"
      justifyContent="center"
      backgroundColor="#091E3B"
    >
      <Box m="0">
        <Box backgroundImage={mountain} style={{height:"50%"}}>
          <Heading as="h1" textAlign="center" fontSize="2xl" mt="100px" color="#FFB46D">
            Welcome aboard the Explorer, {account?.name}!
          </Heading>
          <Text fontSize="xl" textAlign="center" mt="30px">
            The Green Alternative
          </Text>
          <Text fontSize="xl" textAlign="center" mt="30px">
            {guest && guest.id == ''
              ? 'Hang on, we are creating a guest account for you...'
              : !guest
              ? 'There was a problem retrieving your guest account...'
              : room && room.roomNumber == ''
              ? 'Hang on, your room is not ready yet...'
              : !room
              ? 'There was a problem retrieving your room...'
              : 'Your room number is ' + room?.roomNumber}
          </Text>
        </Box>



        <Box className="buttonContainer">
          <Box className='startButtonBox'>
            <Button className="startButton" variant="outline" style={{fontSize:"30px"}}>
              <Link to="/ExcursionOverview">Excursions</Link>
            </Button>
            <img className="startButtonImage" src={startExcursions}></img>
          </Box>

          
          <Box className='startButtonBox'>
            <Button className="startButton" variant="outline" style={{fontSize:"30px"}}>
              Spa
            </Button>
            <img className="startButtonImage" src={startSpa}></img>
          </Box>
          
          <Box className='startButtonBox'>
            <Button className="startButton" variant="outline" style={{fontSize:"30px"}}>
              Restaurants
            </Button>
            <img className="startButtonImage" src={startRestaurant}></img>
          </Box>
          
          <Box className='startButtonBox'>
            <Button className="startButton" variant="outline" style={{fontSize:"30px"}}>
              Shop
            </Button>
            <img className="startButtonImage" src={startShop}></img>
          </Box>
          
        </Box>
        <Box className="buttonContainer">
        <Box className='startButtonBox'>
            <Button className="startBottomButton" variant="outline" style={{fontSize:"18px", backgroundColor: "#FFB46D"}}>
              Restaurants
            </Button>
          </Box>
          
          <Box className='startButtonBox'>
            <Button className="startBottomButton" variant="outline" style={{fontSize:"18px", backgroundColor: "#FFB46D", padding:"0px, 24px, 0px, 24px"}}>
              Shop
            </Button>
          </Box>
        </Box>
      </Box>
    </Flex>
  );
};

const CopyToClipboardButton = (text: string, label?: string) => {
  const copyToClipboard = () => {
    navigator.clipboard.writeText(text);
  };

  return (
    <Tooltip label={label ?? 'Copy to clipboard'}>
      <Button
        w="fit-content"
        p="4"
        px="4px"
        colorScheme="blue"
        borderRadius="10px"
        m="0 auto"
        mt="8"
        fontWeight="bold"
        color="white"
        fontSize="l"
        onClick={copyToClipboard}
      >
        📄
      </Button>
    </Tooltip>
  );
}

export default Start;
