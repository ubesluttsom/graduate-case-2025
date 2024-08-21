import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Start from "./pages/Start";
import Excursions from "./pages/Excursions";

const App = () => {

  return (
    <Routes>
      <Route path="/" element={<Start />}>
      </Route>
      <Route path="/Excursions" element={<Excursions />}>
      </Route>
    </Routes>
  )

}

export default App;
